export class DoctroPostDto {
    username: string = '';
    password: string = '';
    email: string = '';
    name: string = '';
    middleName: string = '';
    surname: string = '';
    gender: string = '';
    dateOfBirth: Date = new Date(); 
    specialization: string = '';

    constructor() {
        
    }
}