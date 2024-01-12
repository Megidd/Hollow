package hollow

import (
	"github.com/deadsy/sdfx/obj"
	"github.com/deadsy/sdfx/render"
	"github.com/deadsy/sdfx/sdf"
)

type Accuracy int

const (
	Low Accuracy = iota + 1
	Medium
	High
)

// Save output STL.
func Hollow(pth string, thcknss float64, accrcy Accuracy, pthOut string) error {
	imprtMsh, _, _, err := hollow(pth, thcknss, accrcy)
	if err != nil {
		return err
	}

	return render.SaveSTL(pthOut, imprtMsh)
}

// Just call logic without saving output STL.
func hollow(pth string, thcknss float64, accrcy Accuracy) ([]*sdf.Triangle3, sdf.SDF3, sdf.SDF3, error) {
	// create the SDF from the mesh
	// WARNING: It will only work on non-intersecting closed-surface(s) meshes.
	imprtSdf, err := obj.ImportSTL(pth, 20, 3, 5)
	if err != nil {
		return nil, nil, nil, err
	}

	insdSdf := sdf.Offset3D(imprtSdf, -thcknss) // Pass negative value for inside.

	insdMsh := render.ToTriangles(insdSdf, render.NewMarchingCubesUniform(cellCountOfAxis(accrcy)))

	insdMshFlp := FlipNormal(insdMsh)

	imprtMsh, err := render.LoadSTL(pth)
	if err != nil {
		return nil, nil, nil, err
	}

	imprtMsh = append(imprtMsh, insdMshFlp...)

	return imprtMsh, imprtSdf, insdSdf, nil
}

func FlipNormal(trngls []*sdf.Triangle3) []*sdf.Triangle3 {
	for i, trngl := range trngls {
		// Swap 2nd and 3rd vertices.
		trngls[i] = &sdf.Triangle3{trngl[0], trngl[2], trngl[1]}
	}
	return trngls
}

func cellCountOfAxis(accrcy Accuracy) int {
	cycleMap := map[Accuracy]int{Low: 20, Medium: 50, High: 80}
	return cycleMap[accrcy]
}
