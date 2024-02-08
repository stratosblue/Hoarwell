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


| Method   | EchoCount | ContentSize | Mean       | Error     | StdDev    | Ratio | Gen0     | Allocated | Alloc Ratio |
|--------- |---------- |------------ |-----------:|----------:|----------:|------:|---------:|----------:|------------:|
| Hoarwell | 1         | 32          |   223.1 us |   1.41 us |   1.32 us |  0.68 |        - |   1.35 KB |        0.38 |
| DotNetty | 1         | 32          |   326.8 us |   4.50 us |   3.99 us |  1.00 |   0.4883 |   3.52 KB |        1.00 |
|          |           |             |            |           |           |       |          |           |             |
| Hoarwell | 1         | 1024        |   222.4 us |   1.06 us |   0.99 us |  0.68 |   0.9766 |   7.16 KB |        0.77 |
| DotNetty | 1         | 1024        |   325.0 us |   3.84 us |   3.59 us |  1.00 |   1.4648 |   9.36 KB |        1.00 |
|          |           |             |            |           |           |       |          |           |             |
| Hoarwell | 100       | 32          |   522.3 us |  10.27 us |  17.99 us |  0.07 |  13.6719 |  87.11 KB |        0.31 |
| DotNetty | 100       | 32          | 7,735.9 us | 243.51 us | 706.48 us |  1.00 |  31.2500 |  277.6 KB |        1.00 |
|          |           |             |            |           |           |       |          |           |             |
| Hoarwell | 100       | 1024        |   687.1 us |  13.74 us |  23.70 us |  0.08 | 115.2344 | 676.83 KB |        0.78 |
| DotNetty | 100       | 1024        | 8,259.9 us |  86.94 us |  77.07 us |  1.00 | 140.6250 | 867.85 KB |        1.00 |
