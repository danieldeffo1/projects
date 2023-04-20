#include "CopyAssembly.h"


CopyAssembly::CopyAssembly(std::string arg1, std::string arg2)
{
    source = arg1;
    destination = arg2;
}

void CopyAssembly::copy()
{
    sourceFileFound = SRC.check(source);
    if (!sourceFileFound) exit(0);
    destinationFileFound = DST.check(destination);
    if (source == destination) {
        std::cout << "Ошибка. Пути одинаковы!\n";
        exit(0);
    }

    if (destinationFileFound) {
        CopyMenu CM;
        CM.menu(CS, source, destination);
    }
    else
        CS.copySuccessful(source, destination);
}
