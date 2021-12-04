// @flow

//import fse from 'fs-extra';
import { execSync } from 'child_process';

import { format, config, appPath, _log  } from './utlis';
const llog = str => _log('Bundle','INFO', str);

(() => {

    //let app = await preBuild();
    //const appPath = app.path;
    const packageDir = format(`${appPath}/Tizen/shared/res`);
    llog(`Output Bundle Path: ${packageDir}`);

    //bundle: '        --dev false'
    // const RN = format(`${appPath}/node_modules/react-native/packager/`);

    // replaceTizen(format(`${RN}defaults.js`), /react-native-windows/g, 'react-native-tizen-dotnet');
    // replaceTizen(format(`${RN}defaults.js`), /windows/g, 'tizen');
    // replaceTizen(format(`${RN}src/node-haste/lib/getPlatformExtension.js`), /web/g, 'tizen');
    

    // function replaceTizen(file, reg, key) {
    //     let data = fse.readFileSync(file, 'utf8');
    //     let result = data.replace(reg, key);
    //     fse.writeFileSync(file, result, 'utf8');
    // }

    function mode(cmd) {
        if (!cmd) {
            return false;
        }
        if (cmd.toLowerCase() === 'dev') {
            return true;
        }

        return config.mode === 'Debug'? 'true' : 'false';
    }
    llog(`React Native will Bundle file with Platfrom: tizen`);
    
    //make bundle comand
    const SPACE = ' ';
    let arg1 = 'node' + SPACE + format(`${appPath}/node_modules/react-native/local-cli/cli.js`) + SPACE + 'bundle --entry-file index.tizen.js';
    let arg2 = ' --bundle-output' + SPACE + format(`${packageDir}/index.tizen.bundle`);
    let arg3 = ' --platform tizen --assets-dest' + SPACE + format(`${packageDir}/assets/`);
    let arg4 = ' --dev ' + mode();

    execSync(arg1 + arg2 + arg3 + arg4, { stdio: [0, 1, 2] });

})();

//module.exports = packager;