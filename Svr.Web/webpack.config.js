//'use strict';

const webpack = require('webpack');
var path = require('path');
const bundleFolder = "./wwwroot/assets/";
const srcFolder = "./App/";


module.exports = {
    entry: 
        { regionlist: srcFolder + "regionlist.jsx" }
    ,
    //entry: {

    //iskovoezajvlenie: "./Scripts/components/IskovoeZajvlenie.jsx"
    //"./src/app.jsx"
    //}, // входная точка - исходный файл
    devtool: "source-map",
    output: {
        filename: "[name].js",
        publicPath: 'assets/',
        path: path.resolve(__dirname, bundleFolder)
    },
    //output: {
    //    path: path.resolve(__dirname, './Scripts'),     // путь к каталогу выходных файлов - папка public
    //    publicPath: '/Scripts/',
    //    filename: "[name].bundle.js"       // название создаваемого файла
    //},
    module: {
        rules: [
            {
                test: /\.jsx$/,
                exclude: /(node_modules)/,
                loader: "babel-loader",
                query: {
                    presets: ["es2015", "stage-0", "react"]
                }
            }
        ]
    },

    //module: {
    //    rules: [   //загрузчик для jsx
    //        {
    //            test: /\.jsx?$/, // определяем тип файлов
    //            exclude: /(node_modules)/,  // исключаем из обработки папку node_modules
    //            loader: "babel-loader",   // определяем загрузчик
    //            options: {
    //                presets: [
    //                    "@babel/preset-env",
    //                    "@babel/preset-react",
    //                    {
    //                        'plugins': [
    //                            '@babel/plugin-proposal-class-properties'
    //                        ]
    //                    }
    //                ]    // используемые плагины
    //            }
    //        },
    //        {
    //            test: /\.css$/,
    //            use: [
    //                {
    //                    loader: "style-loader"
    //                },
    //                {
    //                    loader: "css-loader",
    //                    options: {
    //                        sourceMap: true,
    //                        modules: true,
    //                        localIdentName: "[local]___[hash:base64:5]"
    //                    }
    //                },
    //                {
    //                    loader: "less-loader"
    //                }
    //            ]
    //        }
    //    ]
    //}
    plugins: [
    ]
};