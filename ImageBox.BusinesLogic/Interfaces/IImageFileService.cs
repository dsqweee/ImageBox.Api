using ImageBox.Shared.Interfaces;

namespace ImageBox.BusinessLogic.Interfaces;

public interface IImageFileService
{
    /// <summary>
    /// Получение информации о формате сохранения изображений
    /// </summary>
    /// <returns>Возвращает формат энкодинга (jpg,png и тд.)</returns>
    string GetEncodingFormat();

    /// <summary>
    /// Создать изображение в файловой системе
    /// </summary>
    /// <param name="fileData">Интерфейс передачи MemoryStream файла</param>
    /// <returns>Возвращает кортеж из двух строк. Путь файла и его Хэш (рандомный)</returns>
    Task<(string? imagePath, string? imageHash)> CreateImageFileAsync(IFileData fileData);

    /// <summary>
    /// Удалить изображение из файловой системы по заданному пути
    /// </summary>
    /// <param name="filePath">Путь файла</param>
    /// <returns>Возвращает bool, содержащий статус удаления</returns>
    bool DeleteImageFileAsync(string filePath);

    /// <summary>
    /// Достать изображение из файловой системы по заданному пути
    /// </summary>
    /// <param name="filePath">Путь файла</param>
    /// <returns>Возвращает массив byte, содержащий изображение</returns>
    Task<byte[]> GetImageFileAsync(string filePath);

    /// <summary>
    /// Проверка файла на наличие изображения
    /// </summary>
    /// <param name="fileData">Интерфейс передачи MemoryStream файла</param>
    /// <returns>Возвращает bool, содержащий статус, является ли файл изображением</returns>
    bool VerifyImageAsync(IFileData fileData);
}