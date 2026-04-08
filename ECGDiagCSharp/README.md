# ECG Diagnosis Library - C# Conversion

## 概述 (Overview)

这是将 ECGdiagA20170629 C++代码转换为 C# 的项目。原始代码是一个用于心电图（ECG）诊断分析的医疗库。

This project converts the ECGdiagA20170629 C++ code to C#. The original code is a medical library for electrocardiogram (ECG) diagnosis and analysis.

## 项目结构 (Project Structure)

```
ECGDiagCSharp/
├── ECGDiag.sln                    # Visual Studio解决方案文件
└── ECGDiag/
    ├── ECGDiag.csproj              # C#项目文件
    ├── Core/
    │   └── EcgDiagDefines.cs       # 核心数据结构定义
    ├── VhEcgDiag.cs                # 主诊断类
    ├── Filters/                     # 滤波器类 (待转换)
    ├── Coding/                      # 诊断编码类 (待转换)
    ├── Templates/                   # 模板分析类 (待转换)
    └── Math/                        # 数学工具类 (待转换)
```

## 已完成的转换 (Completed Conversions)

### 1. 核心数据结构 (Core Data Structures)
- ✅ `EcgDiagDefines.cs` - 从 `EcgDiagDefines.h` 转换
  - `VH_ECGparm` - ECG公共参数结构
  - `VH_ECGlead` - 导联参数结构
  - `VH_ECGbeat` - 心搏参数结构
  - `VH_ECGinfo` - ECG信息类
  - `VH_Template` - 模板数据类
  - `EcgLeadIndex` - 导联索引枚举
  - `InitialEcgLeadInfo` - 导联初始化信息

### 2. 主诊断类 (Main Diagnosis Class)
- ✅ `VhEcgDiag.cs` - 从 `vhEcgDiag.h/cpp` 转换
  - 主类 `CvhEcgDiag` 及其所有公共接口
  - V导联索引枚举 `Vindex`
  - 所有参数获取方法（uV振幅、ms时程、ST段等）
  - 诊断编码检索方法
  - 波形类型判断方法

## 待转换的组件 (Components to be Converted)

### 高优先级 (High Priority)
1. **ECGprop类** (`ECGpropEx.h/cpp`)
   - 多导联ECG处理
   - 自动分析功能
   - 参数计算

2. **诊断编码类** (`EcgCodeVH.h/cpp`, `EcgCodeMC.h/cpp`)
   - Minnesota编码
   - VH编码
   - 诊断规则引擎

3. **模板分析类** (`ECGtempl.h/cpp`)
   - `MultiLead_Templates` - 多导联模板
   - `ECG_Template` - ECG模板
   - `QRS_Complex` - QRS波群分析
   - `PT_Wave` - P波和T波分析

4. **逐拍分析类** (`ECGbeats.h/cpp`)
   - `MultiLead_ECG` - 多导联ECG分析
   - 心搏检测
   - 节律分析

### 中优先级 (Medium Priority)
5. **滤波器类** (`Filters/`)
   - `Filters.h/cpp` - 基础滤波器
   - `hspecgFilters.h/cpp` - 专用滤波器
   - `CFilters.h/mm` - C滤波器包装

6. **数学工具类** (`MyMath.h/cpp`)
   - 数学辅助函数
   - 信号处理工具

7. **编码基础类** (`EcgCodeBase.h/cpp`, `CodeEx2.h/cpp`)
   - 编码管理器
   - 基础编码类

8. **采样率转换** (`Double/DoubleSampleRate.h/mm`)
   - 双倍采样率转换

## 关键转换说明 (Key Conversion Notes)

### C++ 到 C# 的映射 (C++ to C# Mapping)

| C++ | C# |
|-----|-----|
| `short` | `short` |
| `BOOL` | `bool` |
| `BYTE` | `byte` |
| `DWORD` | `uint` |
| `char *` | `string` or `char[]` |
| `short **` | `short[][]` |
| Pointers | References or nullable types |
| Manual memory management | Garbage collection |
| `#define` macros | `const` or `static readonly` |
| C-style unions | `[StructLayout(LayoutKind.Explicit)]` |

### 特殊处理 (Special Handling)

1. **内存布局 (Memory Layout)**
   - 使用 `[StructLayout]` 属性确保与原始C++结构体兼容
   - 使用 `[FieldOffset]` 处理union类型

2. **数组处理 (Array Handling)**
   - 固定大小数组转换为属性方法（`GetOnOff()`, `SetOnOff()`）
   - 多维数组使用锯齿数组 `short[][]`

3. **全局变量 (Global Variables)**
   - 转换为实例字段或静态单例模式

4. **析构函数 (Destructors)**
   - C#使用垃圾回收，不需要显式释放内存
   - 如需要，实现 `IDisposable` 接口

## 构建和使用 (Build and Usage)

### 构建项目 (Building the Project)

```bash
cd ECGDiagCSharp
dotnet build
```

### 运行测试 (Running Tests)

```bash
dotnet test
```

### 使用示例 (Usage Example)

```csharp
using VH.ECGDiag;
using VH.ECGDiag.Core;

// 创建诊断对象
var diag = new CvhEcgDiag();

// 创建ECG诊断
short chNumber = 12;  // 12导联
short seconds = 10;   // 10秒数据
short samplerate = 500;  // 500 Hz采样率
double uVperbit = 4.88;  // 每位微伏数

short[][] ecgData = LoadECGData(); // 加载ECG数据

bool success = diag.CreateEcgDiag(chNumber, ecgData, seconds, samplerate, uVperbit);

if (success)
{
    // 执行编码诊断
    diag.EcgCode('M', 45); // 男性，45岁

    // 获取诊断结果
    short mcCount = diag.GetMcCodeCount();
    short vhCount = diag.GetVhCodeCount();

    // 获取参数
    short hr = diag.HR();
    short qt = diag.QT();
    short qtc = diag.QTc();

    Console.WriteLine($"Heart Rate: {hr} bpm");
    Console.WriteLine($"QT: {qt} ms");
    Console.WriteLine($"QTc: {qtc} ms");
}
```

## 系统要求 (System Requirements)

- .NET 6.0 或更高版本
- Visual Studio 2022 或 Visual Studio Code
- 推荐：Windows 10/11 或 Linux 发行版

## 开发状态 (Development Status)

当前版本：**Alpha 0.1**

- ✅ 核心数据结构已完成
- ✅ 主诊断类框架已完成
- ⏳ ECGprop类待完成
- ⏳ 诊断编码类待完成
- ⏳ 滤波器类待完成
- ⏳ 单元测试待添加

## 许可证 (License)

本项目保留原始代码的所有权利。VH Medical 版权所有。

## 贡献 (Contributing)

如需贡献代码或报告问题，请提交Issue或Pull Request。

## 联系方式 (Contact)

原作者：Du Xiaodong, VH Medical
转换项目：2026年4月

## 注意事项 (Important Notes)

⚠️ **医疗软件警告**: 本软件用于ECG诊断分析，在临床使用前必须经过充分验证和监管机构批准。

⚠️ **Medical Software Warning**: This software is for ECG diagnosis analysis. It must be thoroughly validated and approved by regulatory authorities before clinical use.

## 技术参考 (Technical References)

- Minnesota Code (MC Code) - ECG诊断编码标准
- VH Code - 心电图诊断编码系统
- 12导联ECG标准分析方法
