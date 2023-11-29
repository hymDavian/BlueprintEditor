"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
//用于分析和导出某个ts文件下的所有特定class的所有函数信息
var ts = require("typescript");
var fs = require("fs");
var options = {
    target: ts.ScriptTarget.ES5,
    module: ts.ModuleKind.CommonJS,
    allowJs: true
};
var checker = null;
/**判断是否为导出节点 */
function isNodeExported(node) {
    return ((ts.getCombinedModifierFlags(node) & ts.ModifierFlags.Export) !== 0 ||
        (!!node.parent && node.parent.kind === ts.SyntaxKind.SourceFile));
}
/**判断是否为类成员函数 */
// function isClassFunction(node: ts.Node): boolean {
//     return ts.isMethodDeclaration(node) //是一个成员方法
//         && ts.isFunctionLike(node) //是一个函数
//         && ts.isClassElement(node)//是一个类元素
// }
/** 将符号序列化为json对象 */
function serializeSymbol(symbol) {
    var typestr = checker.typeToString(checker.getTypeOfSymbolAtLocation(symbol, symbol.valueDeclaration));
    return {
        name: symbol.getName(),
        documentation: ts.displayPartsToString(symbol.getDocumentationComment(checker)),
        type: typestr
    };
}
/** 序列化类的符号信息 */
function serializeClass(symbol) {
    var details = serializeSymbol(symbol);
    // 取得构造定义
    var constructorType = checker.getTypeOfSymbolAtLocation(symbol, symbol.valueDeclaration);
    details.constructors = constructorType
        .getCallSignatures()
        .map(serializeSignature);
    return details;
}
/** 序列化调用签名 */
function serializeSignature(signature) {
    return {
        parameters: signature.parameters.map(serializeSymbol),
        returnType: checker.typeToString(signature.getReturnType()),
        documentation: ts.displayPartsToString(signature.getDocumentationComment(checker))
    };
}
function getInFileClassFuncInfos(files) {
    var _a;
    var ret = [];
    var program = ts.createProgram(files, options);
    checker = program.getTypeChecker();
    var output = [];
    //将一个函数的信息返回
    function readFunctionInfoToOut(node, cls) {
        if (ts.isMethodDeclaration(node)) { //是一个函数
            // console.log(">>>>>");
            // console.log("getJSDocCommentsAndTags:", ts.getJSDocCommentsAndTags(node))
            // console.log("getJSDocTags:", ts.getJSDocTags(node))
            // console.log("<<<<<");
            var symbol = checker.getSymbolAtLocation(node.name);
            var ser = serializeClass(symbol);
            ser.class = cls;
            output.push(ser);
        }
    }
    //先定义文件操作方式
    function visit(node) {
        if (!isNodeExported(node)) { //非导出节点不进行操作
            return;
        }
        if (ts.isClassDeclaration(node) && node.name) {
            var clsName_1 = node.name.escapedText.toString();
            //这是一个顶级的类，获取它的符号
            try {
                ts.forEachChild(node, function (node) { readFunctionInfoToOut(node, clsName_1); });
            }
            catch (error) {
                console.error("errormsg:" + error);
            }
            // let symbol = checker.getSymbolAtLocation(node.name);
            // if (symbol) {
            //     output.push(serializeClass(symbol));
            // }
        }
        else if (ts.isModuleDeclaration(node)) {
            //这是一个命名空间，访问其子级
            ts.forEachChild(node, visit);
        }
    }
    //访问所有文件
    for (var _i = 0, _b = program.getSourceFiles(); _i < _b.length; _i++) {
        var sourceFile = _b[_i];
        if (!sourceFile.isDeclarationFile) {
            var fileSTartLinr = sourceFile.getFullText().split('\n')[0];
            console.log(fileSTartLinr);
            if (fileSTartLinr.includes("//>>#BPExportFile#<<")) {
                //对所有文件执行操作
                ts.forEachChild(sourceFile, visit);
            }
        }
    }
    var _loop_1 = function (func) {
        var cls = func.class;
        var clsObj = ret.find(function (val) { return val.className == cls; });
        if (clsObj == null) {
            clsObj = {
                className: cls,
                funcs: []
            };
            ret.push(clsObj);
        }
        var paramsList = [];
        var paramsTypes = [];
        var Signature = func.constructors[0]; //函数签名
        (_a = Signature.parameters) === null || _a === void 0 ? void 0 : _a.forEach(function (val) {
            paramsList.push(val.name);
            paramsTypes.push(val.type);
        });
        var retType = Signature.returnType; //返回类型
        var funcdes = Signature.documentation; //函数描述
        var f = {
            functionName: func.name,
            paramsList: paramsList,
            paramsTypes: paramsTypes,
            retType: retType,
            funcdes: funcdes
        };
        clsObj.funcs.push(f);
    };
    //写入文本 4缩进
    for (var _c = 0, output_1 = output; _c < output_1.length; _c++) {
        var func = output_1[_c];
        _loop_1(func);
    }
    fs.mkdir("outjson", function (err) { });
    fs.writeFileSync("outjson/classDef.json", JSON.stringify(ret, undefined, 4));
    // console.log(JSON.stringify(ret, undefined, 4));
    return ret;
}
// console.log("argc:" + process.argv + ",end");
getInFileClassFuncInfos(process.argv.slice(2)); //F:\MyGit\Test\TS\test.ts process.argv.slice(2)
