#pragma once
#include <iostream>
#include <string>
#include "CopySuccessful.h"
class CopyMenu
{
    int choice;
    std::string beg;
    std::string end;
    int indexOfTheSymbolPosition;
    void canceling();
    void replaceTheFile(CopySuccessful CS, std::string source, std::string destination);
    void addBegin(CopySuccessful CS, std::string source, std::string destination);
    void addEnd(CopySuccessful CS, std::string source, std::string destination);
public:
    void menu(CopySuccessful CS, std::string source, std::string destination);
};
