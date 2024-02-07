//TODO 完成读取方法，eg: 使用长度头部的字符串读取，

//using System.Buffers;

//namespace Hoarwell.Enhancement.Buffers;

///// <summary>
///// 序列结构化读取器
///// </summary>
//public ref struct SequenceStructuredReader
//{
//    #region Public 属性

//    /// <summary>
//    /// 内部使用的 <see cref="SequenceReader{T}"/>
//    /// </summary>
//    public SequenceReader<byte> SequenceReader { get; }

//    #endregion Public 属性

//    #region Public 构造函数

//    /// <inheritdoc cref="SequenceStructuredReader"/>
//    public SequenceStructuredReader(SequenceReader<byte> sequenceReader)
//    {
//        SequenceReader = sequenceReader;
//    }

//    /// <inheritdoc cref="SequenceStructuredReader"/>
//    public SequenceStructuredReader(ReadOnlySequence<byte> readOnlySequence) : this(new SequenceReader<byte>(readOnlySequence))
//    {
//    }

//    #endregion Public 构造函数

//}
