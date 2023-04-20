#pragma once
#include <string>
#include <iostream>
#include "CheckSource.h"
#include "CheckDestination.h"
#include "CopySuccessful.h"
#include "CopyMenu.h"

class CopyAssembly
{
    std::string source, destination;
    CheckSource SRC;
    CheckDestination DST;
    CopySuccessful CS;
    bool sourceFileFound, destinationFileFound;
public:
    CopyAssembly(std::string arg1, std::string arg2);
    void copy();
};
