"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var fs = require("fs");
function readType(file) {
    fs.readFile(file, function (err, data) {
        if (!err) {
            console.log(typeof JSON.parse(data.toString()));
        }
        else {
            console.log(err);
        }
    });
}
readType(process.argv[2]);
