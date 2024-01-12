package hollow

import (
	"github.com/deadsy/sdfx/render"
	"github.com/deadsy/sdfx/sdf"
	v3 "github.com/deadsy/sdfx/vec/v3"
)

func Infill(pth string, thcknss float64, accrcy Accuracy, pthOut string) error {
	imprtMsh, _, insdSdf, err := hollow(pth, thcknss, accrcy)
	if err != nil {
		return err
	}

	infllMsh, err := infillGyroid(insdSdf, accrcy)
	if err != nil {
		return err
	}

	imprtMsh = append(imprtMsh, infllMsh...)

	return render.SaveSTL(pthOut, imprtMsh)
}

func infillGyroid(shape sdf.SDF3, accrcy Accuracy) ([]*sdf.Triangle3, error) {
	cycle := cycleOfSide(accrcy)

	min := shape.BoundingBox().Min
	max := shape.BoundingBox().Max

	xK := (max.X - min.X) / float64(cycle)
	yK := (max.Y - min.Y) / float64(cycle)
	zK := (max.Z - min.Z) / float64(cycle)

	gyroidSdf, err := sdf.Gyroid3D(v3.Vec{X: xK, Y: yK, Z: zK})
	if err != nil {
		return nil, err
	}

	// A gyroid is defined over all of 3d space,
	// and we turn it into a finite object by intersecting with a 3D model.
	// To get a gyroid region with a shape of 3D model.
	gyroidFiniteSdf := sdf.Intersect3D(shape, gyroidSdf)

	cellCountOfLongestAxis := cellCountOfAxisGyroid(accrcy)
	gyroidFiniteMsh := render.ToTriangles(gyroidFiniteSdf, render.NewMarchingCubesUniform(cellCountOfLongestAxis))

	return gyroidFiniteMsh, nil
}

func cycleOfSide(accrcy Accuracy) int {
	cycleMap := map[Accuracy]int{Low: 8, Medium: 15, High: 25}
	return cycleMap[accrcy]
}

func cellCountOfAxisGyroid(accrcy Accuracy) int {
	cycleMap := map[Accuracy]int{Low: 40, Medium: 60, High: 80}
	return cycleMap[accrcy]
}
