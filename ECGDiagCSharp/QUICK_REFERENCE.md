# ECGdiagA20170629 C# 转换快速参考

## 已转换文件清单

### ✅ 已完成

| 原始C++文件 | C#文件 | 状态 |
|-------------|--------|------|
| EcgDiagDefines.h | Core/EcgDiagDefines.cs | ✅ 完成 |
| vhEcgDiag.h | VhEcgDiag.cs | ✅ 完成 |
| vhEcgDiag.cpp | VhEcgDiag.cs | ✅ 完成 |
| - | ECGDiag.csproj | ✅ 新建 |
| - | ECGDiag.sln | ✅ 新建 |

### ⏳ 待转换 (按优先级排序)

#### 第一优先级：核心算法
| 原始C++文件 | 预计C#文件 | 行数 | 优先级 |
|-------------|-----------|------|--------|
| ECGpropEx.h/cpp | ECGpropEx.cs | ~930 | 🔴 最高 |
| ECGtempl.h/cpp | Templates/ECGTemplate.cs | ~4,060 | 🔴 最高 |
| ECGbeats.h/cpp | Templates/ECGBeats.cs | ~3,435 | 🔴 最高 |

#### 第二优先级：诊断编码
| 原始C++文件 | 预计C#文件 | 行数 | 优先级 |
|-------------|-----------|------|--------|
| EcgCodeVH.h/cpp | Coding/EcgCodeVH.cs | ~3,770 | 🟠 高 |
| EcgCodeMC.h/cpp | Coding/EcgCodeMC.cs | ~2,900 | 🟠 高 |
| EcgCodeBase.h/cpp | Coding/EcgCodeBase.cs | ~1,050 | 🟠 高 |

#### 第三优先级：支持模块
| 原始C++文件 | 预计C#文件 | 行数 | 优先级 |
|-------------|-----------|------|--------|
| Filters/Filters.h/cpp | Filters/Filters.cs | ~600 | 🟡 中 |
| Filters/hspecgFilters.h/cpp | Filters/HspecgFilters.cs | ~1,300 | 🟡 中 |
| CFilters/CFilters.h/mm | Filters/CFilters.cs | ~1,340 | 🟡 中 |
| MyMath.h/cpp | Math/MyMath.cs | ~320 | 🟡 中 |
| CodeEx2.h/cpp | Coding/CodeEx2.cs | ~395 | 🟡 中 |
| Double/DoubleSampleRate.h/mm | Double/DoubleSampleRate.cs | ? | 🟢 低 |

## 关键API映射表

### CvhEcgDiag 主类方法

| C++ 方法 | C# 方法 | 说明 |
|----------|---------|------|
| `CreateEcgDiag(...)` | `CreateEcgDiag(...)` | 创建ECG诊断 |
| `EcgCode(...)` | `EcgCode(...)` | 执行诊断编码 |
| `GetFirstMcode()` | `GetFirstMcode()` | 获取第一个MC编码 |
| `GetNextMcode()` | `GetNextMcode()` | 获取下一个MC编码 |
| `GetFirstRcode()` | `GetFirstRcode()` | 获取第一个VH编码 |
| `GetNextRcode()` | `GetNextRcode()` | 获取下一个VH编码 |
| `HR()` | `HR()` | 获取心率 |
| `RR()` | `RR()` | 获取RR间期 |
| `QT()` | `QT()` | 获取QT间期 |
| `QTc()` | `QTc()` | 获取校正QT间期 |

### 数据类型映射

| C++ 类型 | C# 类型 | 说明 |
|----------|---------|------|
| `BOOL` | `bool` | 布尔值 |
| `BYTE` | `byte` | 无符号8位 |
| `short` | `short` | 有符号16位 |
| `long` | `int` | 有符号32位 |
| `DWORD` | `uint` | 无符号32位 |
| `double` | `double` | 双精度浮点 |
| `char *` | `string` / `char[]` | 字符串/字符数组 |
| `short **` | `short[][]` | 二维数组（锯齿数组） |

## 编译和测试

### 编译项目
```bash
cd ECGDiagCSharp
dotnet build
```

### 清理项目
```bash
dotnet clean
```

### 发布项目
```bash
dotnet publish -c Release -o publish
```

## 代码示例

### 创建诊断对象
```csharp
using VH.ECGDiag;

var diag = new CvhEcgDiag();
```

### 设置V导联索引
```csharp
diag.SetVindex(CvhEcgDiag.Vindex.V1, 6);
diag.SetVindex(CvhEcgDiag.Vindex.V2, 7);
```

### 创建ECG诊断
```csharp
short[][] ecgData = LoadYourECGData();
bool success = diag.CreateEcgDiag(
    chNumber: 12,
    dataIn: ecgData,
    seconds: 10,
    samplerate: 500,
    uVperbit: 4.88
);
```

### 执行诊断编码
```csharp
if (success)
{
    diag.EcgCode('M', 45, 0); // 男性，45岁
}
```

### 获取诊断参数
```csharp
var hr = diag.HR();           // 心率
var qt = diag.QT();           // QT间期
var qtc = diag.QTc();         // 校正QT
var qrs = diag.QRS();         // QRS时程
var rv5 = diag.RV5();         // V5导联R波振幅

Console.WriteLine($"HR: {hr} bpm");
Console.WriteLine($"QT: {qt} ms");
Console.WriteLine($"QTc: {qtc} ms");
```

### 获取诊断编码
```csharp
short mcCount = diag.GetMcCodeCount();
short vhCount = diag.GetVhCodeCount();

// 遍历MC编码
short code = diag.GetFirstMcode();
while (code != 0)
{
    string? codeStr = CvhEcgDiag.McCode(code);
    Console.WriteLine($"MC Code: {codeStr}");
    code = diag.GetNextMcode();
}

// 遍历VH编码
code = diag.GetFirstRcode();
while (code != 0)
{
    string? codeStr = CvhEcgDiag.VhCode(code);
    Console.WriteLine($"VH Code: {codeStr}");
    code = diag.GetNextRcode();
}
```

## 注意事项

### 当前限制
1. ⚠️ ECGprop类为占位符，核心算法尚未实现
2. ⚠️ 诊断编码功能需要EcgCodeVH/MC类实现
3. ⚠️ 滤波器功能尚未转换
4. ⚠️ 缺少单元测试

### 内存管理
- C#使用垃圾回收，无需手动释放内存
- 不再需要delete/delete[]操作
- 大数据建议使用ArrayPool或Memory<T>优化

### 线程安全
- 原始C++代码未考虑线程安全
- C#版本建议每个线程使用独立的CvhEcgDiag实例
- 或者添加适当的锁机制

### 性能考虑
- 数组访问可能比C++略慢
- 考虑使用Span<T>优化性能关键路径
- 大量数值计算可考虑使用unsafe代码块

## 下一步开发计划

1. **Phase 1**: 转换ECGprop核心类
   - MultiLead_ECG类
   - MultiLead_Templates类
   - 自动分析算法

2. **Phase 2**: 转换模板分析类
   - ECG_Template类
   - QRS_Complex类
   - PT_Wave类

3. **Phase 3**: 转换诊断编码类
   - CvhCode类
   - CmcCode类
   - 编码规则引擎

4. **Phase 4**: 转换滤波器类
   - 基础滤波器
   - 专用滤波器

5. **Phase 5**: 测试和优化
   - 单元测试
   - 性能测试
   - 文档完善

## 技术支持

如有问题或建议，请查看：
- `README.md` - 项目总体介绍
- `CONVERSION_SUMMARY.md` - 详细转换总结
- 原始C++代码注释

## 许可和使用

本项目保留原始VH Medical代码的所有权利。
医疗诊断软件，使用前请确保符合相关法规要求。
