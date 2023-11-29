//>>#BPExportFile#<<
import { BlueprintLogic } from "./BPObject";

/**节点可用动作函数库 ()=>void */
export class FunctionLibrary {
    /**测试函数 */
    public testCasteSkill_out(logic: BlueprintLogic, damage: number) {
        console.log(logic.id + "释放攻击，造成" + damage + "伤害")
    }
    /**设置蓝图变量 */
    public setBPVariable_out(logic: BlueprintLogic, key: string, value: any) {
        console.log(`设置${logic.id}的${key}为${value}`);
        logic.setVariable(key, value, true);
    }
    /**比较两数大小 */
    public compareNumber_out(logic: BlueprintLogic, a: number, b: number): boolean {
        return a > b;
    }
    /**判断某个值是否存在 */
    public checkValueIsTrue_out(logic: BlueprintLogic, v: any): boolean {
        return !(v == null);
    }
    /**数字相加 */
    public numberAdd_out(logic: BlueprintLogic, a: number, b: number) {
        return a + b;
    }
    /**数字相减 */
    public numberReduce_out(logic: BlueprintLogic, a: number, b: number) {
        return a - b;
    }
    /**数字相乘 */
    public numberMultiply_out(logic: BlueprintLogic, a: number, b: number) {
        return a * b;
    }
    /**数字相除 */
    public numberDivision_out(logic: BlueprintLogic, a: number, b: number) {
        return a / b;
    }
    /**数字取反 */
    public numberNegation_out(logic: BlueprintLogic, a: number) {
        return -a;
    }


}
