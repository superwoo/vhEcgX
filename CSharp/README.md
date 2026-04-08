# C# Conversion of vhEcgX

This directory contains the C# conversion of the iOS Objective-C vhEcgX application.

## Overview

The original application is an ECG (electrocardiogram) data collection app that:
- Connects to Bluetooth Low Energy (BLE) devices
- Receives ECG data from BLE peripherals
- Processes the raw ADC data with filtering
- Displays the ECG waveform on a scrollable view

## Converted Files

The following Objective-C files have been converted to C#:

1. **AppDelegate.cs** - Main application delegate
2. **BLEManager.cs** - Bluetooth Low Energy manager (singleton pattern)
3. **HomeViewController.cs** - Main screen showing available BLE devices
4. **CollectDataViewController.cs** - Screen for collecting and displaying ECG data
5. **LeadView.cs** - Custom view for rendering ECG waveforms with grid
6. **Main.cs** - Application entry point

## Framework Requirements

To build this C# project, you'll need:

### Xamarin.iOS Framework
This is a Xamarin.iOS application that requires:
- **Xamarin.iOS** or **.NET for iOS** (formerly Xamarin.iOS)
- **Visual Studio for Mac** or **Visual Studio 2022** (Windows with Mac build host)

### NuGet Packages Required
While most iOS frameworks are built into Xamarin.iOS, you may want:
- No additional NuGet packages are strictly required for the core functionality
- Optional: Auto Layout library like **Cirrious.FluentLayout** to replace Masonry constraints

### iOS Frameworks Used
- **UIKit** - User interface
- **Foundation** - Basic data types and utilities
- **CoreBluetooth** - Bluetooth Low Energy communication
- **CoreData** - Data persistence (used in AppDelegate)
- **CoreGraphics** - Drawing ECG waveforms

## Key Conversion Notes

### Language Differences Handled

1. **Objective-C to C# Syntax**
   - Properties: `@property` → C# properties with get/set
   - Method names: Converted from Objective-C selector style to C# naming conventions
   - Protocols: `@protocol` → C# interfaces (prefixed with `I`)
   - Blocks: Objective-C blocks → C# lambda expressions/delegates

2. **Memory Management**
   - Removed manual `retain`/`release` (handled by C# garbage collection)
   - Strong references maintained where needed

3. **Delegates and Protocols**
   - `BLEManagerDelegate` → `IBLEManagerDelegate` interface
   - TableView delegates implemented as `IUITableViewDelegate` and `IUITableViewDataSource`

4. **Collections**
   - `NSMutableArray` → `List<T>` (generic C# collections)
   - Type safety improved with generics

5. **Singleton Pattern**
   - Converted from `dispatch_once` to thread-safe C# pattern with lock

6. **Auto Layout**
   - Masonry constraints replaced with simple frame-based layout
   - Can be upgraded to use Cirrious.FluentLayout or native constraints if needed

### Known Limitations

1. **CFilters Class**
   - The `CLowpassFilter2` class is a placeholder
   - Original implementation is in C++ (CFilters.h)
   - Needs to be converted from C++ to C# or accessed via P/Invoke

2. **Layout System**
   - Currently using frame-based layout instead of Masonry
   - Should be updated to use Auto Layout constraints for better device support

3. **Testing Required**
   - The code has been converted but not compiled or tested
   - Bluetooth functionality needs testing with actual BLE devices
   - ECG data processing and display needs validation

## Building the Project

To create a working Xamarin.iOS project:

1. Create a new Xamarin.iOS Single View App project in Visual Studio
2. Add all the C# files to the project
3. Configure Info.plist with Bluetooth usage description:
   ```xml
   <key>NSBluetoothAlwaysUsageDescription</key>
   <string>This app needs Bluetooth to connect to ECG devices</string>
   <key>NSBluetoothPeripheralUsageDescription</key>
   <string>This app needs Bluetooth to connect to ECG devices</string>
   ```
4. Implement or port the CFilters/CLowpassFilter2 class
5. Build and test on a physical iOS device (Bluetooth doesn't work in simulator)

## BLE Configuration

The app connects to BLE devices with these UUIDs:
- **Service UUID**: `6E400001-B5A3-F393-E0A9-E50E24DCCA9E`
- **UART RX UUID**: `6E400002-B5A3-F393-E0A9-E50E24DCCA9E`
- **UART TX UUID**: `6E400003-B5A3-F393-E0A9-E50E24DCCA9E`

## ECG Data Processing

The app processes 24-bit ADC data with the following parameters:
- **ADC Gain**: 6x
- **Reference Voltage**: 2.4V
- **Sample Rate**: 500 Hz
- **Display Scale**: 10mm/mV, 25mm/second
- **Low-pass Filter**: 40Hz cutoff at 500Hz sample rate

## Next Steps

To complete this conversion:
1. Create a Xamarin.iOS project structure
2. Add project file (.csproj)
3. Convert or link the C++ filter library (CFilters)
4. Add storyboard or XIB files if needed (currently uses programmatic UI)
5. Test with actual BLE ECG devices
6. Add error handling and user feedback
7. Implement proper resource management

## License

Copyright © 2018 谷山丰. All rights reserved.
