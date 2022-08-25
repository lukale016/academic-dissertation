import { Patient } from 'src/app/models/Patient';
import { Doctor } from 'src/app/models/Doctor';
export class Appointment {
    id?: number;
    doctorRef: string;
    doctor: Doctor;
    patientRef: string;
    patient: Patient;
    scheduledTime: Date;
    lengthInMins: number;

    /**
     * @summary One arg copy ctor
     * @summary 4 args: doctor username, patient username, scheduled time, duration
     */
     constructor(...args: any[]) {
        if(args.length == 0)
        {
            this.doctor = new Doctor();
            this.doctorRef = "";
            this.patient = new Patient();
            this.patientRef = "";
            this.scheduledTime = new Date();
            this.lengthInMins = 0;
            return;
        }
        if(args.length == 1)
        {
            let appointment : Appointment = args[0] as Appointment;
            this.id = appointment.id;
            this.doctor = appointment.doctor;
            this.doctorRef = appointment.doctorRef;
            this.patient = appointment.patient;
            this.patientRef = appointment.patientRef;
            this.scheduledTime = appointment.scheduledTime;
            this.lengthInMins = appointment.lengthInMins;
            return;
        }
        if(args.length == 4)
        {
            this.doctor = new Doctor();
            this.doctorRef = args[0] as string;
            this.patient = new Patient();
            this.patientRef = args[1] as string;
            this.scheduledTime = args[2] as Date;
            this.lengthInMins = args[3] as number;
            return;
        }
        console.log("Something went wrong in appointment ctor");
        this.doctor = new Doctor();
        this.doctorRef = "";
        this.patient = new Patient();
        this.patientRef = "";
        this.scheduledTime = new Date();
        this.lengthInMins = 0;
    }
}