#include "CommandChecking.h"

CommandChecking::CommandChecking() {
    RC.read();
    if (RC.command == "copy") {
        if (RC.arg.size() != 2) {
            std::cout << "������ ���������� �������!\n\n";
            exit(0);
        }
        CopyAssembly C�(RC.arg[0], RC.arg[1]);
        C�.copy();
    }
    else std::cout << "������� �� �������!\n\n";
}
