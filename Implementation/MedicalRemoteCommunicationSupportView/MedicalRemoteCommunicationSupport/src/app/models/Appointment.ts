/// <reference path="Models.ts" />
namespace models
{
    export interface Appointment {
        doctor: string;
        patient: string;
        date: Date;
        startTime: Date;
        endTime: Date;
    }
}