const webpack = require('webpack');
const path = require('path');
const srcPath = './src';
const distPath = './wwwroot';
const ExtractTextPlugin = require('extract-text-webpack-plugin');

module.exports = {
  entry: {
    'index': `${srcPath}/index.tsx`
  },
  output: {
    path: distPath,
    filename: 'index.js'
  },
  module: {
    loaders: [
      {
        test: /\.tsx?$/,
        loader: 'ts'
      },
      {
        test: /\.scss$/,
        loader: ExtractTextPlugin.extract({
          loader: ['css', 'sass']
        })
      }
    ]
  },

  resolve: {
    extensions: ['.ts', '.js', '.tsx', '.css', '.scss']
  },

  plugins: [
    new ExtractTextPlugin({
      filename: 'index.css',
      allChunks: true
    })
  ],

  devtool: 'source-map'
};
