//用于分析和导出某个ts文件下的所有特定class的所有函数信息
import * as ts from "typescript";
import * as fs from "fs";
type FuncDescription = {
    functionName: string,//函数名
    paramsList: string[],//参数列表名
    paramsTypes: string[], //参数列表需求类型组 0-string 1-number 2-bool
    retType: string, //返回类型
    funcdes: string //函数描述
}
type OutFileJson = {
    className: string,
    funcs: FuncDescription[]
}


interface DocEntry {
    class?: string,
    name?: string;
    fileName?: string;
    documentation?: string;
    type?: string;
    constructors?: DocEntry[];
    parameters?: DocEntry[];
    returnType?: string;
}
const options: ts.CompilerOptions = {
    target: ts.ScriptTarget.ES5,
    module: ts.ModuleKind.CommonJS,
    allowJs: true
}
let checker: ts.TypeChecker = null;
/**判断是否为导出节点 */
function isNodeExported(node: ts.Node): boolean {
    return (
        (ts.getCombinedModifierFlags(node as ts.Declaration) & ts.ModifierFlags.Export) !== 0 ||
        (!!node.parent && node.parent.kind === ts.SyntaxKind.SourceFile)
    );
}
/**判断是否为类成员函数 */
// function isClassFunction(node: ts.Node): boolean {
//     return ts.isMethodDeclaration(node) //是一个成员方法
//         && ts.isFunctionLike(node) //是一个函数
//         && ts.isClassElement(node)//是一个类元素
// }
/** 将符号序列化为json对象 */
function serializeSymbol(symbol: ts.Symbol): DocEntry {
    const typestr = checker.typeToString(
        checker.getTypeOfSymbolAtLocation(symbol, symbol.valueDeclaration!)
    );

    return {
        name: symbol.getName(),
        documentation: ts.displayPartsToString(symbol.getDocumentationComment(checker)),
        type: typestr
    };
}
/** 序列化类的符号信息 */
function serializeClass(symbol: ts.Symbol) {
    let details = serializeSymbol(symbol);

    // 取得构造定义
    let constructorType = checker.getTypeOfSymbolAtLocation(
        symbol,
        symbol.valueDeclaration!
    );
    details.constructors = constructorType
        .getCallSignatures()
        .map(serializeSignature);
    return details;
}
/** 序列化调用签名 */
function serializeSignature(signature: ts.Signature) {
    return {
        parameters: signature.parameters.map(serializeSymbol),
        returnType: checker.typeToString(signature.getReturnType()),
        documentation: ts.displayPartsToString(signature.getDocumentationComment(checker))
    };
}

function getInFileClassFuncInfos(files: string[]): OutFileJson[] {
    const ret: OutFileJson[] = [];
    const program = ts.createProgram(files, options);
    checker = program.getTypeChecker();
    const output: DocEntry[] = [];

    //将一个函数的信息返回
    function readFunctionInfoToOut(node: ts.Node, cls: string) {
        if (ts.isMethodDeclaration(node)) {//是一个函数
            let symbol = checker.getSymbolAtLocation(node.name);
            const ser = serializeClass(symbol);
            ser.class = cls;
            output.push(ser);
        }
    }

    //先定义文件操作方式
    function visit(node: ts.Node) {
        if (!isNodeExported(node)) {//非导出节点不进行操作
            return;
        }
        if (ts.isClassDeclaration(node) && node.name) {
            const clsName = node.name.escapedText.toString();
            //这是一个顶级的类，获取它的符号
            try {

                ts.forEachChild(node, node => { readFunctionInfoToOut(node, clsName); });
            } catch (error) {
                console.error("errormsg:" + error);
            }

            // let symbol = checker.getSymbolAtLocation(node.name);
            // if (symbol) {
            //     output.push(serializeClass(symbol));
            // }

        } else if (ts.isModuleDeclaration(node)) {
            //这是一个命名空间，访问其子级
            ts.forEachChild(node, visit);


        }
    }
    //访问所有文件
    for (const sourceFile of program.getSourceFiles()) {

        if (!sourceFile.isDeclarationFile) {
            const fileSTartLinr = sourceFile.getFullText().split('\n')[0];
            console.log(fileSTartLinr);
            if (fileSTartLinr.includes("//>>#BPExportFile#<<")) {
                //对所有文件执行操作
                ts.forEachChild(sourceFile, visit);
            }
        }
    }
    //写入文本 4缩进
    for (const func of output) {
        const cls = func.class;
        let clsObj: OutFileJson = ret.find(val => { return val.className == cls });
        if (clsObj == null) {
            clsObj = {
                className: cls,
                funcs: []
            }
            ret.push(clsObj);
        }

        const paramsList: string[] = [];
        const paramsTypes: string[] = [];
        const Signature: DocEntry = func.constructors[0];//函数签名
        Signature.parameters?.forEach(val => {
            paramsList.push(val.name);
            paramsTypes.push(val.type);
        });
        const retType = Signature.returnType;//返回类型
        const funcdes = Signature.documentation;//函数描述
        const f: FuncDescription = {
            functionName: func.name,
            paramsList: paramsList,
            paramsTypes: paramsTypes,
            retType: retType,
            funcdes: funcdes
        }
        clsObj.funcs.push(f);
    }
    fs.mkdir("outjson", err => { })
    fs.writeFileSync("outjson/classDef.json", JSON.stringify(ret, undefined, 4));
    // console.log(JSON.stringify(ret, undefined, 4));

    return ret;
}
// console.log("argc:" + process.argv + ",end");
getInFileClassFuncInfos(process.argv.slice(2))//F:\MyGit\Test\TS\test.ts process.argv.slice(2)