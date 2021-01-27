"use strict"
{
    let path = require('path');
	let webpack = require('webpack');
    let bundleFolder = "wwwroot/SSA2SRT/";

    const MiniCssExtractPlugin = require('mini-css-extract-plugin');
    const CssMinimizerPlugin = require('css-minimizer-webpack-plugin');

    module.exports = {
        entry: {
            'app.bundle.js': "./src/app.js"
        },
        output: {
            filename: '[name]',
            path: path.resolve(__dirname, bundleFolder)
        },
        module: {

            rules: [
                {
                    test: /\.(ttf|eot|woff|woff2|svg|png|jpg|gif|ico)(\?v=[0-9]\.[0-9]\.[0-9])?$/,
                    use: [
                        'url-loader',
                    ],
                },
                {
                    test: /\.css$/,
                    use: [
                        MiniCssExtractPlugin.loader,
                        'css-loader',
                    ],
                },
            ],
        },
        optimization: {
            minimize: true,
            minimizer: [
                '...',
                new CssMinimizerPlugin()
            ],
        },
        plugins: [
            new MiniCssExtractPlugin({
                filename: 'app.bundle.css',
            }),
            new webpack.ProvidePlugin({
			    $: "jquery",
			    jQuery: "jquery",
                "window.jQuery": "jquery"
            })
		]
    };
}