// Note this only includes basic configuration for development mode.
// For a more comprehensive configuration check:
// https://github.com/fable-compiler/webpack-config-template

var path = require("path");
const webpack = require("webpack");

var mode = hasArg("prod") ? "production" : "development";

module.exports = env => ({
    mode: mode,
    entry: "./src/Root.fs.js",
    output: {
        path: path.join(__dirname, "./public"),
        filename: "bundle.js",
    },
    devServer: {
        static: {
            directory: path.resolve(__dirname, "./public"),
            publicPath: "/",
        },
        port: 8080,
    },
    module: {
    },
    plugins: [
      new webpack.DefinePlugin({
        VERSION: JSON.stringify(env.version ?? 'dev'),
      })
    ]
});


function hasArg(arg) {
  return arg instanceof RegExp
      ? process.argv.some(x => arg.test(x))
      : process.argv.indexOf(arg) !== -1;
}