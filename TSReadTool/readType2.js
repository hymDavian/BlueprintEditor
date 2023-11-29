const fs = require('fs-extra');
const commander = require('commander');

/**
 * 将 JSON 数据转换为 TypeScript 类型定义
 * @param {Object} object - 要转换的 JSON 对象
 * @param {string} [name=JsonType] - 转换后的类型名称
 * @param {string} [namespace] - 转换后的命名空间
 * @returns {string} - 转换后的 TypeScript 类型定义字符串
 */
function jsonToTs(object, name = 'JsonType', namespace) {
    const getType = value => {
        let typeRes = ``;
        if (Array.isArray(value)) {
            value.forEach(item => {

                let subType = getType(item);
                if (typeRes.split('|').indexOf(subType) < 0) {
                    typeRes += subType
                    typeRes += "|"
                }
            })
            typeRes = typeRes.substring(0, typeRes.length - 1)
            return `(${typeRes})[]`;
        }
        if (typeof value === 'object' && value !== null) {
            const props = Object.entries(value)
                .map(([key, val]) => `${key}: ${getType(val)}`)
                .join('; ');
            return `{ ${props} }`;
        }
        return typeof value;
    };

    const type = getType(object);

    const declaration = `interface ${name} ${type}`;

    return namespace ? `namespace ${namespace} { \r\n ${declaration} \r\n}` : declaration;
}

/**
 * 读取文件并解析成 JSON 对象
 * @param {string} path - 文件路径
 * @returns {Promise<Object>} - JSON 对象
 */
async function readJson(path) {
    const content = await fs.readFile(path, 'utf8');
    return JSON.parse(content);
}

/**
 * 将 TypeScript 类型定义字符串写入文件
 * @param {string} content - TypeScript 类型定义字符串
 * @param {string} path - 文件路径
 * @returns {Promise<void>}
 */
async function writeTs(content, path) {
    await fs.writeFile(path, content, 'utf8');
}

/**
 * 将 JSON 数据转换为 TypeScript 类型定义
 * @param {string} inputPath - 输入 JSON 文件路径
 * @param {string} outputPath - 输出 TypeScript 文件路径
 * @param {string} [options.name=JsonType] - 转换后的类型名称
 * @param {string} [options.namespace] - 转换后的命名空间
 * @param {boolean} [options.noFile] - 不将 TypeScript 类型定义保存为文件
 * @returns {Promise<void>}
 */

async function jsonToTsFile(inputPath, outputPath, options) {
    const { name, namespace, noFile } = options
    try {
        const object = await readJson(inputPath);
        const type = jsonToTs(object, name, namespace);
        if (noFile) {
            console.log(type);
        } else {
            await writeTs(type, outputPath);
            console.log(`Type definition saved to ${outputPath}`);
        }
    } catch (err) {
        console.error(err.message);
    }
}

const program = new commander.Command();

program
    .arguments('<input> <output>')
    .option('--no-file', 'do not save to file')
    .option('-s, --namespace <namespace>', 'type namespace')
    .option('-n, --name <name>', 'type name', 'JsonType')
    .action(jsonToTsFile);

program.parse(process.argv);

