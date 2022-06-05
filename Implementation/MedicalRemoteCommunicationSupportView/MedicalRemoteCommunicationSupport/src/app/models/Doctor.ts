/// <reference path="Models.ts" />
namespace models
{
    export interface Doctor {
        username: string;
        password: string;
        name: string;
        middleName: string;
        surname: string;
        gender: string;
        dateOfBirth: Date;
        isDoctor: boolean;
        appointmentDatesListKey: string;
        appointments: string[];
        messageListPriorityListKey: string;
        messageListPriorityList: string[];
        fullName: string;
        requestListKey: string;
        requests: Request[];
        patientListKey: string;
        specialization: string;
        startTime: string;
        endTime: string;
    }
}