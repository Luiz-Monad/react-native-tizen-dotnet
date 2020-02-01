# react-native-tizen-dotnet

**react-native-tizen-dotnet** is a React Native framework for developer to build Tizen.NET apps on Tizen.  
It provides the same fundamental UI components and user experience with Tizen native Apps. Also it provides a easy and fast debugging way with Hot Module Reloading feature.

`react-native-tizen-dotnet` is based on [react-native v0.42](https://github.com/facebook/react-native/tree/0.42-stable) and [react-native-windows v0.42](https://github.com/Microsoft/react-native-windows/tree/0.42-stable).  

## Compatibility

`react-native-tizen-dotnet` may not support all React Native Components and APIs.  
Review our [docs](Docs/doc-index.md) for information about currently supported Components and APIs.

## Source Code Directory Structure

```diff
react-native-tizen-dotnet
├── Devtools/ - Development tool .
├── Docs/ - Components & APIs Documents.
├── Example/ - Example of react-native-tizen-dotnet App.
└── Framework/ - react-native-tizen-dotnet Framework.
    ├── react-native-tizen/ - JS Library.
    └── ReactNet/ - Native Framework based on .NET Code.
```

![Framework](./Docs/img/Framework.PNG)

## Getting Started

### Developer Dependencies
-  [Node.js](https://nodejs.org/en/download/)
-  [.NET Core SDK](https://dotnet.microsoft.com/download)

### Install

`sudo npm i -g create-react-native-tizen-app`

### Usage

-  `create-react-native-tizen-app myTizenApp`
-   `cd myTizenApp`
-   `vim package.json` *change "tvip": "192.168.100.1" to your Tv IP*
-   `yarn bundle` *for release mode (npm run bundle)*
-   `yarn bundle --dev` *for dev mode, js file not ugly (npm run bundle --dev)*
-   `yarn package` *packaging tpk for Tizen (npm run package)*
-   `yarn launch` *launch tpk to Tizen TV , Before launch you [Enable Developer Mode on the TV](#Connect-to-TV) to TV) first* (npm run launch)

## Connect to TV

Refer to this website, **Enable Developer Mode on the TV**:  
https://developer.samsung.com/SmartTV/develop/getting-started/using-sdk/tv-device.html

## Running in emulator

> Note: `react-native-tizen-dotnet` depends on the C++ libraries `JSCore` and `yoga`. 
> The emulator has a diffrent CPU Arch than TV devices.
> So we provide diffrent C++ libraries (arm & i586).

When creating a new `react-native-tizen-dotnet` project, the ARM base library path is `your-project\Tizen\lib\`.  
If you want to your app in the emulator, you must replace the ARM library path with the i586 based libraries. 

The paths for `i586` libraries are:  

```
Framework/ReactNet/JSCore/libJSCore_i586.so
Framework/ReactNet/yoga/libyoga_i586.so
```

## Debug

`react-native-tizen-dotnet` supports the same familiar debugging tooling as `react-native` such as `Hot Reloading`, `JS Remote Debugging`.

To enable debugging, you need follow these steps:

1. Modify the `package.json` file in your app.
   
   Set `config.mode` to *Debug* (Default setting is *Release*)
   
   ```js
   // package.json
   {
     // ...
     "config": {
       // ...
       "mode": "Debug"
     },
   // ...
   }
   ```
   
2. Launch the debug server on your local machine.
   
   ``` shell
   npm run server
   ```
   
   The server should now be running on port `8081`.
   
3. Package your app and launch on TV

   - `yarn package`
   - `yarn launch`
   
   *Note: Your app should launch and connect to debug server automatically.*
   
4. Use the TV Remote's `Red` or `A` button to configure debug settings.
   
   *Note: If your app can't connect to debug server, you can set host IP in this same manner.*
   
   *Note: If `input panel` is hard to use, suggest using real keybroad*
