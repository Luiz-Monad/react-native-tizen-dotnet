// @ts-check
require('source-map-support').install();

module.exports = {
  platforms: {
    tizen: {
      linkConfig: () => null,
      projectConfig: () => null,
      dependencyConfig: () => null,
      npmPackageName: 'react-native-tizen',
    },
  },
};
