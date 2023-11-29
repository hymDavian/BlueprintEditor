import * as fs from "fs";

function getTypeStr(o: any): string {
    const keyMap: { [k: string]: string } = {};
    for (const k in o) {
        const tyStr = typeof o[k];
        if (tyStr == "object") {
            keyMap[k] = getTypeStr(o[k]);
        }
        else {
            switch (tyStr) {
                case "bigint": keyMap[k] = "bigint"; break;
                case "string": keyMap[k] = "string"; break;
                case "number": keyMap[k] = "number"; break;
                case "boolean": keyMap[k] = "boolean"; break;
                default: keyMap[k] = "any"; break;
            }
        }
    }
    let ret = "{";
    for (const k in keyMap) {
        const v = keyMap[k];
        ret += `\n"${k}":${v}`;
    }
    ret += "\n}"
    return ret;
}

function readType(file: string) {
    fs.readFile(file, (err, data: Buffer) => {
        if (!err) {
            const str = data.toString();
            let o = JSON.parse(str);
            const typestr = getTypeStr(o);//类型表示字符串
            fs.mkdir("ty", err => { });
            fs.writeFileSync("ty/tys.ts", typestr);

        }
        else {
            console.log(err)
        }
    });
}

readType(process.argv[2]);

