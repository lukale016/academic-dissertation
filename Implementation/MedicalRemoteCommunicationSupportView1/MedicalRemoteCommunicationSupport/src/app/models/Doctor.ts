import { Patient } from './Patient';
export class Doctor {
    username: string;
    password: string;
    name: string;
    middleName: string;
    surname: string;
    gender: string;
    dateOfBirth: Date;
    isDoctor: boolean;
    appointments: string[];
    messageListPriority: string[];
    fullName: string;
    requests: Request[];
    patients: Patient[];
    specialization: string;
    startTime: string;
    endTime: string;

    /**
     * @summary One arg copy ctor
     * @summary 10 arg: username, password, name, middle, surname, gender, birth, specialization, start time, end time
     */
    constructor(...args: any[]) {
        if(args.length == 0)
        {
            this.username = "";
            this.password = ""
            this.name = "";
            this.middleName = "";
            this.surname = "";
            this.fullName = "";
            this.gender = "";
            this.dateOfBirth = new Date();
            this.isDoctor = false;
            this.appointments = [];
            this.messageListPriority = [];
            this.requests = [];
            this.patients = [];
            this.specialization = "";
            this.startTime = "";
            this.endTime = "";
            return;
        }
        if(args.length == 1)
        {
            let doctor : Doctor = args[0] as Doctor;
            this.username = doctor.username;
            this.password = doctor.password;
            this.name = doctor.name;
            this.middleName = doctor.middleName;
            this.surname = doctor.surname;
            this.fullName = doctor.fullName;
            this.gender = doctor.gender;
            this.dateOfBirth = doctor.dateOfBirth;
            this.isDoctor = doctor.isDoctor;
            this.appointments = doctor.appointments;
            this.messageListPriority = doctor.messageListPriority;
            this.requests = doctor.requests;
            this.patients = doctor.patients;
            this.specialization = doctor.specialization;
            this.startTime = doctor.startTime;
            this.endTime = doctor.endTime;
            return;
        }
        if(args.length == 11)
        {
            this.username = args[0] as string;
            this.password = args[1] as string
            this.name = args[2] as string;
            this.middleName = args[3] as string;
            this.surname = args[4] as string;
            this.fullName = "";
            this.gender = args[5] as string;
            this.dateOfBirth = args[6] as Date;
            this.isDoctor = true;
            this.appointments = [];
            this.messageListPriority = [];
            this.requests = [];
            this.patients = [];
            this.specialization = args[7] as string;
            this.startTime = args[8] as string;
            this.endTime = args[9] as string;
            return;
        }
        console.log("Something went wrong in doctor ctor");
        this.username = "";
        this.password = ""
        this.name = "";
        this.middleName = "";
        this.surname = "";
        this.fullName = "";
        this.gender = "";
        this.dateOfBirth = new Date();
        this.isDoctor = false;
        this.appointments = [];
        this.messageListPriority = [];
        this.requests = [];
        this.patients = [];
        this.specialization = "";
        this.startTime = "";
        this.endTime = "";
    }
}