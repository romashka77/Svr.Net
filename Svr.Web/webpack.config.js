//https://codeburst.io/how-to-use-webpack-in-asp-net-core-projects-a-basic-react-template-sample-25a3681a5fc2
//https://medium.com/front-end-weekly/webpack-by-example-part-3-a4ceaaa6299a
//https://www.youtube.com/watch?v=_GtHmFFxi_8&list=PL0lO_mIqDDFXaDvwLJ6aMnmIt7sdmujKp&index=6
//https://monsterlessons.com/project/lessons/redux-js-vstuplenie
const path = require('path');
const webpack = require('webpack');
const TerserJSPlugin = require("terser-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const OptimizeCSSAssetsPlugin = require("optimize-css-assets-webpack-plugin");

const devMode = process.env.NODE_ENV !== 'production';
//const UglifyJsPlugin = require('uglifyjs-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const CleanWebpackPlugin = require('clean-webpack-plugin');

module.exports = {
    context: path.resolve(__dirname, 'Source'),
    devtool: devMode ? 'cheap-eval-source-map' : 'source-map',
    mode: 'production',
    entry: {
        claimlist: './claimlist',
        common: './common'
        //site: './site',
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
        ]
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
            'window.jQuery': 'jquery',
            Popper: ['popper.js', 'default']
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
                test: /.(css|scss)$/,
                use: [MiniCssExtractPlugin.loader, "css-loader", "postcss-loader", "sass-loader"]
            },
            //{
            //    test: /\.css$/,
            //    use: [
            //        {
            //            loader: MiniCssExtractPlugin.loader,
            //            options: {
            //                // you can specify a publicPath here
            //                // by default it use publicPath in webpackOptions.output
            //                publicPath: '/dist/'
            //            }
            //        },
            //        'css-loader'
            //    ]
            //},
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