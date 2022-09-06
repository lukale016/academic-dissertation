import { Appointment } from './Appointment';
import { Doctor } from 'src/app/models/Doctor';
import { Patient } from './Patient';
import { Topic } from './Topic';

export class SuperUser {
    username: string;
    password: string;
    name: string;
    middleName: string;
    surname: string;
    gender: string;
    dateOfBirth: Date;
    isDoctor: boolean;
    appointments: Appointment[];
    fullName: string;
    specialization: string;
    startTime: string;
    endTime: string;
    messageListPriority: string[];
    createdTopics: Topic[];
    sentRequests: {username: string, specialization: string}[];
    requests: {username: string, fullName: string}[];
    patients: {username: string, fullName: string, skypeId: string}[];
    myDoctors: {username: string, fullName: string, skypeId: string}[];

    constructor(user: Patient | Doctor | null)
    {
        if(user === null)
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
            this.createdTopics = [];
            this.sentRequests = [];
            this.myDoctors = [];
            return;
        }
        if(!user.isDoctor && user.username != "") {
            let patient : Patient = user as Patient;
            this.username = patient.username;
            this.password = patient.password;
            this.name = patient.name;
            this.middleName = patient.middleName;
            this.surname = patient.surname;
            this.fullName = patient.fullName;
            this.gender = patient.gender;
            this.dateOfBirth = patient.dateOfBirth;
            this.isDoctor = patient.isDoctor;
            this.appointments = patient.appointments;
            this.messageListPriority = patient.messageListPriority;
            this.createdTopics = patient.createdTopics;
            this.requests = [];
            this.patients = [];
            this.specialization = "";
            this.startTime = "";
            this.endTime = "";
            this.sentRequests = patient.sentRequests;
            this.myDoctors = patient.myDoctors;
            return;
        }
        else if(user.isDoctor) {
            let doctor : Doctor = user as Doctor;
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
            this.specialization = doctor.specialization;
            this.startTime = doctor.startTime;
            this.endTime = doctor.endTime;
            this.messageListPriority = doctor.messageListPriority;
            this.createdTopics = [];
            this.sentRequests = [];
            this.myDoctors = [];
            this.requests = doctor.requests;
            this.patients = doctor.patients;
            return;
        }
        console.log("Something went wrong in super user ctor");
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
        this.createdTopics = [];
        this.sentRequests = [];
        this.myDoctors = [];
    }
}