#include "CopyMenu.h"

void CopyMenu::canceling()
{
    std::cout << std::endl << "����������� ��������" << std::endl;
    exit(0);
}

void CopyMenu::replaceTheFile(CopySuccessful CS, std::string source, std::string destination) {
    CS.copySuccessful(source, destination);
}

void CopyMenu::addBegin(CopySuccessful CS, std::string source, std::string destination) {
    std::cout << std::endl << "�������� � ������: "; std::cin >> beg;
    int indexOfTheSymbolPosition = (int)destination.rfind('/');
    destination.insert(indexOfTheSymbolPosition + 1, beg);
    CS.copySuccessful(source, destination);
}

void CopyMenu::addEnd(CopySuccessful CS, std::string source, std::string destination) {
    std::cout << std::endl << "�������� � �����: "; std::cin >> end;
    int indexOfTheSymbolPosition = (int)destination.rfind('.');
    if (indexOfTheSymbolPosition > -1) {
        destination.insert(indexOfTheSymbolPosition, end);
    }
    else destination.insert(destination.size(), end);
    CS.copySuccessful(source, destination);
}

void CopyMenu::menu(CopySuccessful CS, std::string source, std::string destination) {
    int choice;
    std::cout << "���� ��� ����������.\n�������� ��������:- \n\t1. �������� ����������� \n\t2. �������� ���� \n\t3. ������������� ����: �������� � ������ \n\t4. ������������� ����: �������� � �����\n������� �����: ";
    std::cin >> choice;
    if (choice == 1) {
        canceling();
    }
    if (choice == 2) {
        replaceTheFile(CS, source, destination);
    }
    if (choice == 3) {
        addBegin(CS, source, destination);
    }
    if (choice == 4) {
        addEnd(CS, source, destination);
    }
}
