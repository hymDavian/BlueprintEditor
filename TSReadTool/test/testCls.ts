//>>#BPExportFile#<<
import { fff } from "./RefTest";

/**
 * 这是测试类TestA
 */
class TestA {

    public vo() {

    }

    /**
     * 测试函数aaa
     * @returns 返回1
     */
    public aaa(a: string, b: boolean): number {
        return 1;
    }

    /**
     * hhhh
     * ffff
     * gggg
     * @asdfff xxxxaaaa
     */
    public static bbb(): boolean {
        return false;
    }
}
/**这是测试类TestC */
export class TestC extends TestA {
    /**
     * 测试函数ccc
     * @returns 返回"ccc"
     */
    public ccc(c: fff): string {
        return "ccc";
    }
}