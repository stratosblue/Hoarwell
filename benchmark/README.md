# Hoarwell.Benchmark

 - 小数据包
 - 本地回环
 - 基本默认配置
 - 串行执行

// * Summary *

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3085/23H2/2023Update/SunValley3)
Intel Core i7-8700 CPU 3.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.101
  [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2


| Method   | EchoCount | ContentSize | Mean        | Error     | StdDev    | Ratio | Gen0     | Allocated | Alloc Ratio |
|--------- |---------- |------------ |------------:|----------:|----------:|------:|---------:|----------:|------------:|
| Hoarwell | 1         | 32          |    212.2 us |   0.63 us |   0.59 us |  0.69 |        - |   1.35 KB |        0.39 |
| DotNetty | 1         | 32          |    308.4 us |   2.21 us |   2.07 us |  1.00 |   0.4883 |   3.49 KB |        1.00 |
|          |           |             |             |           |           |       |          |           |             |
| Hoarwell | 1         | 1024        |    216.8 us |   1.16 us |   1.08 us |  0.65 |   0.9766 |   7.16 KB |        0.77 |
| DotNetty | 1         | 1024        |    335.3 us |   3.75 us |   3.50 us |  1.00 |   1.4648 |   9.26 KB |        1.00 |
|          |           |             |             |           |           |       |          |           |             |
| Hoarwell | 100       | 32          |    515.4 us |   6.28 us |   5.57 us |  0.05 |  13.6719 |  87.14 KB |        0.33 |
| DotNetty | 100       | 32          |  9,706.4 us |  67.20 us |  59.57 us |  1.00 |  31.2500 | 267.96 KB |        1.00 |
|          |           |             |             |           |           |       |          |           |             |
| Hoarwell | 100       | 1024        |    674.1 us |  13.44 us |  27.46 us |  0.07 | 117.1875 | 677.22 KB |        0.77 |
| DotNetty | 100       | 1024        | 10,342.7 us | 156.47 us | 146.37 us |  1.00 | 140.6250 | 879.16 KB |        1.00 |
