require('.pnp.cjs');
const { join } = require('path');

const paths = {
  build: 'build',
  source: 'src/**/*.js',
  sourceRoot: join(__dirname, 'dev-tools'),
};

export default async function (fly) {  
  await fly.watch(paths.source, 'babel');
}

export async function babel(fly, opts) {
  await fly.clear(paths.build)
    .source(opts.src || paths.source)
    .babel({
      presets:['env']
    })
    .target(paths.build);
}

export async function clean(fly) {
  await fly.clear(paths.build);
}