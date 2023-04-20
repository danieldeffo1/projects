#pragma once
#include <vector>
#include <string>
#include <iostream>

class ReadingCommand
{
public:
    std::string command;
    std::vector <std::string> arg;
    void read();
};
