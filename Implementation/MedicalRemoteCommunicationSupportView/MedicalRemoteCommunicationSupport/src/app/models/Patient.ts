/// <reference path="Models.ts" />
namespace models
{
    export interface Patient {
        username: string;
        password: string;
        name: string;
        middleName: string;
        surname: string;
        fullName: string;
        gender: string;
        dateOfBirth: Date;
        isDoctor: boolean;
        appointments: Appointment[];
        messageListPriorityList: string[];
        createdTopics: Topic[];
    }
}