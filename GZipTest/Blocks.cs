namespace GZipTest;
/// <summary>
/// Класс представляет собой простую структуру данных, содержащую идентификатор (ID) и массив байтов (Block).
/// </summary>
public class Blocks
{
    /// <summary>
    /// Конструктор инициализирует новый экземпляр класса с указанным идентификатором и блоком данных
    /// </summary>
    /// <param name="id"></param>
    /// <param name="block"></param>
    public Blocks(int id, byte[] block)
    {
        ID = id;
        Block = block;
    }

    public int ID { get; set; }
    public byte[] Block { get; set; }
}