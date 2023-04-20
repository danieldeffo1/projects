#include "CommandChecking.h"

CommandChecking::CommandChecking() {
    RC.read();
    if (RC.command == "copy") {
        if (RC.arg.size() != 2) {
            std::cout << "Ошибка параметров команды!\n\n";
            exit(0);
        }
        CopyAssembly CА(RC.arg[0], RC.arg[1]);
        CА.copy();
    }
    else std::cout << "Команда не найдена!\n\n";
}
