export class Appointment {
    doctor: string;
    patient: string;
    date: Date;
    startTime: Date;
    endTime: Date;

    /**
     * @summary One arg copy ctor
     * @summary 3 args: owner, description, isDoctorComment
     */
     constructor(...args: any[]) {
        if(args.length == 0)
        {
            this.doctor = "";
            this.patient = "";
            this.date = new Date();
            this.startTime = new Date();
            this.endTime = new Date();
            return;
        }
        if(args.length == 1)
        {
            let appointment : Appointment = args[0] as Appointment;
            this.doctor = appointment.doctor;
            this.patient = appointment.patient;
            this.date = appointment.date;
            this.startTime = appointment.startTime;
            this.endTime = appointment.endTime;
            return;
        }
        if(args.length == 5)
        {
            this.doctor = args[0] as string;
            this.patient = args[1] as string;
            this.date = args[2] as Date;
            this.startTime = args[3] as Date;
            this.endTime = args[4] as Date;
            return;
        }
        console.log("Something went wrong in appointment ctor");
        this.doctor = "";
        this.patient = "";
        this.date = new Date();
        this.startTime = new Date();
        this.endTime = new Date();
    }
}