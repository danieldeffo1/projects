#include "CheckSource.h"

bool CheckSource::check(std::string& source)
{
    for (int i = 0; i < source.size(); i++)
    {
        if (source[i] == '\\')
            source[i] = '/';
    }
    std::ifstream sourceFile(source.c_str(), std::ios::binary);
    if (!sourceFile)
    {
        sourceFile.close();
        std::cout << "\nОшибка. Не удалось найти файл по данному пути: " << source << std::endl;
        return false;
    }
    else {
        sourceFile.close();
        return true;
    }
}
