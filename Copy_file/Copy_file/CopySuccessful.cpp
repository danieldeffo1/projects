#include "CopySuccessful.h"


void CopySuccessful::copySuccessful(std::string source, std::string destination)
{
    std::ifstream sourceFile(source.c_str(), std::ios::binary);
    std::ofstream destinationFile(destination.c_str(), std::ios::binary);

    int len = 4096;
    char* line = new char[len];
    std::cout << "\n\tПожалуйста подождите...\n";
    while (!sourceFile.eof()) {
        sourceFile.read(line, len);
        if (sourceFile.gcount())
            destinationFile.write(line, sourceFile.gcount());
    }
    sourceFile.close();
    destinationFile.close();
    std::cout << std::endl << "\tФайл успешно скопирован\n" << std::endl;
    exit(0);
}
