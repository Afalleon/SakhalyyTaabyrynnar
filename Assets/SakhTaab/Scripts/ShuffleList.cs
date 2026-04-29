using System.Collections.Generic;

public abstract class ShuffleList
{
    // метод для перемешивания списка любого типа <T>
    public static List<T> ShuffleListItems<T>(List<T> inputList)
    {
        // создаем временную копию списка, чтобы не испортить оригинал
        List<T> originalList = new List<T>();
        originalList.AddRange(inputList);

        // будущий список с перемешанными элементами
        List<T> randomList = new List<T>();

        // объект для генерации случайных чисел
        System.Random r = new System.Random();
        int randomIndex = 0;

        // пока во временном списке есть элементы
        while (originalList.Count > 0)
        {
            // выбираем случайный индекс от 0 до текущего количества элементов
            randomIndex = r.Next(0, originalList.Count);

            // добавляем элемент под этим индексом в новый список
            randomList.Add(originalList[randomIndex]);

            // удаляем этот элемент из временного списка, чтобы не выбрать его снова
            originalList.RemoveAt(randomIndex);
        }

        return randomList; // возвращаем итоговый перемешанный список
    }
}