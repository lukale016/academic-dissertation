export class DoctroPostDto {
    username: string = '';
    password: string = '';
    skypeId: string = '';
    name: string = '';
    middleName: string = '';
    surname: string = '';
    gender: string = '';
    dateOfBirth: Date = new Date(); 
    specialization: string = '';

    constructor() {
        
    }
}