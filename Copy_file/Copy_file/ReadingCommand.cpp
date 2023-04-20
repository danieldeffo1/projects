#include "ReadingCommand.h"

void ReadingCommand::read()
{
    std::string buf;
    int k = -1;
    getline(std::cin, buf);
    for (size_t i = 0; i < buf.size(); i++)
    {
        if (buf[i] == ' ') {
            k++;
            arg.push_back("");
        }
        else {
            if (k == -1) command.push_back(buf[i]);
            else arg[k].push_back(buf[i]);
        }
    }
}
