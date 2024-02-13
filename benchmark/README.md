# Hoarwell.Benchmark

 - 小数据包
 - 本地回环
 - 基本默认配置
 - 串行执行

// * Summary *

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
Intel Core i7-8700 CPU 3.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.200
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2


| Method   | EchoCount | DataSize | Mean       | Error     | StdDev      | Median     | Ratio | RatioSD | Gen0    | Allocated | Alloc Ratio |
|--------- |---------- |--------- |-----------:|----------:|------------:|-----------:|------:|--------:|--------:|----------:|------------:|
| Hoarwell | 1         | 32       |   203.6 us |   4.45 us |    13.12 us |   206.2 us |  0.64 |    0.09 |       - |     809 B |        0.24 |
| DotNetty | 1         | 32       |   307.1 us |   2.09 us |     1.96 us |   307.0 us |  1.00 |    0.00 |  0.4883 |    3442 B |        1.00 |
|          |           |          |            |           |             |            |       |         |         |           |             |
| Hoarwell | 1         | 1024     |   209.9 us |   4.15 us |    10.18 us |   209.8 us |  0.64 |    0.06 |  0.2441 |    2792 B |        0.52 |
| DotNetty | 1         | 1024     |   311.7 us |   0.71 us |     0.66 us |   311.8 us |  1.00 |    0.00 |  0.4883 |    5391 B |        1.00 |
|          |           |          |            |           |             |            |       |         |         |           |             |
| Hoarwell | 100       | 32       |   516.2 us |   9.41 us |    11.55 us |   517.2 us |  0.07 |    0.03 |  4.8828 |   31656 B |        0.12 |
| DotNetty | 100       | 32       | 8,489.4 us | 793.50 us | 2,339.64 us | 9,695.1 us |  1.00 |    0.00 | 39.0625 |  263864 B |        1.00 |
|          |           |          |            |           |             |            |       |         |         |           |             |
| Hoarwell | 100       | 1024     |   581.4 us |  10.83 us |    28.34 us |   572.4 us |  0.07 |    0.02 | 37.1094 |  231076 B |        0.51 |
| DotNetty | 100       | 1024     | 8,699.8 us | 372.60 us | 1,098.61 us | 8,605.7 us |  1.00 |    0.00 | 70.3125 |  455133 B |        1.00 |
