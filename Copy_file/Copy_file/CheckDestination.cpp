#include "CheckDestination.h"

bool CheckDestination::check(std::string& destination)
{
    for (int i = 0; i < destination.size(); i++)
    {
        if (destination[i] == '\\')
            destination[i] = '/';
    }
    std::ifstream idestination(destination.c_str());
    if (idestination) {
        idestination.close();
        return true;
    }
    else {
        idestination.close();
        return false;
    }
}
