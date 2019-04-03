//https://codeburst.io/how-to-use-webpack-in-asp-net-core-projects-a-basic-react-template-sample-25a3681a5fc2
//https://medium.com/front-end-weekly/webpack-by-example-part-3-a4ceaaa6299a
//https://www.youtube.com/watch?v=_GtHmFFxi_8&list=PL0lO_mIqDDFXaDvwLJ6aMnmIt7sdmujKp&index=6
//https://monsterlessons.com/project/lessons/redux-js-vstuplenie
//https://monsterlessons.com/project/lessons/reduxjs-combinereducers
const path = require('path');
const webpack = require('webpack');
const TerserJSPlugin = require("terser-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const OptimizeCSSAssetsPlugin = require("optimize-css-assets-webpack-plugin");
const devMode = process.env.NODE_ENV !== 'production';
//const UglifyJsPlugin = require('uglifyjs-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const CleanWebpackPlugin = require('clean-webpack-plugin');
console.log('devMode', devMode);
console.log('process.env.NODE_ENV', process.env.NODE_ENV);
console.log('================================================');
module.exports = {
    context: path.resolve(__dirname, 'Source'),
    //devtool: devMode ? 'cheap-eval-source-map' : 'source-map',
    devtool: 'source-map',
    mode: devMode ? 'development' : 'production',
    entry: {
        claims: './claims',
        common: './common',
        textFirstUpperCase: './textFirstUpperCase',
        site: './site'
        //validation: './validation'
        //claim: './claim'
    },
    output: {
        path: path.resolve(__dirname, 'wwwroot/dist'),
        publicPath: '/dist/',
        filename: '[name].js',
        chunkFilename: '[id].js',
        library: '[name]'
    },
    optimization: {
        minimizer: [
            new TerserJSPlugin({}),
            new OptimizeCSSAssetsPlugin({
                cssProcessorOptions: { map: { inline: false, annotation: true } }
            })
        ],
        //splitChunks: {
        //    chunks: 'all'
        //}
    },
    resolve: {
        extensions: ['.js', '.jsx', '.css']
    },
    plugins: [
        //new webpack.ProgressPlugin(),
        new CleanWebpackPlugin(),
        new MiniCssExtractPlugin({
            filename: '[name].css',
            chunkFilename: '[id].css',
        }),
        new webpack.ProvidePlugin({
            $: 'jquery',
            jQuery: 'jquery',
            'window.jQuery': 'jquery'
            //,
            //Popper: ['popper.js', 'default']
        }),
        new webpack.HotModuleReplacementPlugin()
    ],
    module: {
        rules: [
            {
                test: /\.js?$/,
                exclude: /(node_modules|bower_components)/,//excluded node_modules 
                use: {
                    loader: "babel-loader",
                }
            },
            {
                //test: /.(css|scss)$/,
                test: /.css$/,
                use: [
                    {
                        loader: MiniCssExtractPlugin.loader,
                        options: {
                            name: '[path][name].css'
                        //    publicPath: '/dist/'
                        }
                    }, {
                        //    loader: 'style-loader', // inject CSS to page
                        //}, {
                        loader: 'css-loader', // translates CSS into CommonJS modules
                        options: { url: false, sourceMap: true }
                    },
                    //{
                    //    loader: 'postcss-loader', // Run post css actions
                    //    options: {
                    //        plugins: function () { // post css plugins, can be exported to postcss.config.js
                    //            return [
                    //                require('precss'),
                    //                require('autoprefixer')
                    //            ];
                    //        }
                    //    }
                    //}
                    //, {
                    //    loader: 'sass-loader', // compiles SASS to CSS
                    //options: { sourceMap: true } 
                    //}
                ]
            },
            {
                test: /\.hbs$/,
                exclude: /(node_modules|bower_components)/,
                use: [
                    'handlebars-loder'
                ]
            },
            {
                test: /\.(png|jpg|svg|ttf|eot|woff|woff2)$/,
                use: [
                    {
                        loader: "file-loader",
                        options: {
                            name: "[path][name]-[hash:8].[ext]"
                        }
                    }
                ]
                //loader: 'url-loader',
                //options: {
                //    limit: 10000
                //}
            }
        ]
    }
};