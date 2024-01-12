package main

import (
	"fmt"
	"log"
	"os"
	"strconv"

	"Hollow/logic/hollow"
)

// Command line example:
// ./main.exe hollow ../sdfx/files/teapot.stl true 0.5 3 infilled.stl
// ./main.exe hollow ../sdfx/files/teapot.stl false 0.5 3 hollowed.stl
func main() {
	var err error
	err = command(os.Args[1:])
	if err != nil {
		log.Fatal(err)
	}
}

func command(args []string) error {
	if len(args) != 5 {
		return fmt.Errorf("usage: wrong argument count")
	}

	pth := args[0]

	isInfill, err := strconv.ParseBool(args[1])
	if err != nil {
		return fmt.Errorf(fmt.Sprintf("invalid boolean value: %s", args[1]))
	}

	thcknss, err := strconv.ParseFloat(args[2], 64)
	if err != nil {
		return fmt.Errorf(fmt.Sprintf("invalid float value: %s", args[2]))
	}

	accrcy, err := strconv.Atoi(args[3])
	if err != nil {
		return fmt.Errorf(fmt.Sprintf("invalid integer value: %s", args[3]))
	}

	pthOut := args[4]

	fmt.Println("Input file:", pth)
	fmt.Println("Is infill needed:", isInfill)
	fmt.Println("Thickness:", thcknss)
	fmt.Println("Accuracy:", accrcy)
	fmt.Println("Output file:", pthOut)

	if isInfill {
		return hollow.Infill(pth, thcknss, hollow.Accuracy(accrcy), pthOut)
	} else {
		return hollow.Hollow(pth, thcknss, hollow.Accuracy(accrcy), pthOut)
	}
}
