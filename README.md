# Viewmodel Offset

A [BepInEx](https://github.com/BepInEx/BepInEx) 5 plugin for **Zumbi Blocks 2** that allows you to customize the first-person viewmodel (weapon/arms) position via configurable X, Y, and Z offsets.

## Features

- **Configurable Offsets:** Adjust the viewmodel position along all three axis (Right/Left, Up/Down, Forward/Backward) through the BepInEx configuration file.
- **Smooth Transitions:** Offset changes are applied smoothly using lerp interpolation.
- **Working ADS:** Automatically returns to the default offset when aiming down sights (ADS).

## Installation

Download the latest DLL at the [releases](https://github.com/tbv001/viewmodeloffset/releases) page, and then copy the DLL into your BepInEx `plugins` folder:

```
Zumbi Blocks 2 Open Alpha\BepInEx\plugins\ViewmodelOffset.dll
```

## Configuration

On first launch, a configuration file is generated at:

```
Zumbi Blocks 2 Open Alpha\BepInEx\config\com.theblackvoid.viewmodeloffset.cfg
```

## License

This project is licensed under the **MIT License** - see the [LICENSE](https://github.com/tbv001/viewmodeloffset/blob/main/LICENSE) file for details.
