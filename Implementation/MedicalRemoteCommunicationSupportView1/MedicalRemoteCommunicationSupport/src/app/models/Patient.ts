import { Appointment } from "./Appointment";
import { Topic } from "./Topic";

export class Patient {
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
    messageListPriority: string[];
    createdTopics: Topic[];
    chats: {username: string, fullName: string}[];

    /**
     * @summary One arg copy ctor
     * @summary 7 arg: username, password, name, middle, surname, gender, birth
     */
    constructor(...args: any[]) {
        if(args.length == 0)
        {
            this.username = "";
            this.password = "";
            this.name = "";
            this.middleName = "";
            this.surname = "";
            this.fullName = "";
            this.gender = "";
            this.dateOfBirth = new Date();
            this.isDoctor = false;
            this.appointments = [];
            this.messageListPriority = [];
            this.createdTopics = [];
            this.chats = [];
            return;
        }
        if(args.length == 1)
        {
            let patient : Patient = args[0] as Patient;
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
            this.chats = patient.chats;
            return;
        }
        if(args.length == 7)
        {
            this.username = args[0] as string;
            this.password = args[1] as string
            this.name = args[2] as string;
            this.middleName = args[3] as string;
            this.surname = args[4] as string;
            this.fullName = "";
            this.gender = args[5] as string;
            this.dateOfBirth = args[6] as Date;
            this.isDoctor = false;
            this.appointments = [];
            this.messageListPriority = [];
            this.createdTopics = [];
            this.chats = [];
            return;
        }
        console.log("Something went wrong in patient ctor");
        this.username = "";
        this.password = "";
        this.name = "";
        this.middleName = "";
        this.surname = "";
        this.fullName = "";
        this.gender = "";
        this.dateOfBirth = new Date();
        this.isDoctor = false;
        this.appointments = [];
        this.messageListPriority = [];
        this.createdTopics = [];
        this.chats = [];
    }
}