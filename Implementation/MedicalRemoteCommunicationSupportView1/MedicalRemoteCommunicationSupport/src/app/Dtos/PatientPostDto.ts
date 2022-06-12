import { DoctroPostDto } from './DoctorPostDto';
export class PatientPostDto {
    username: string = '';
    password: string = '';
    email: string = '';
    name: string = '';
    middleName: string = '';
    surname: string = '';
    gender: string = '';
    dateOfBirth: Date = new Date();

    constructor(docDto: DoctroPostDto) {
        this.username = docDto.username;
        this.password = docDto.password;
        this.email = docDto.email;
        this.name = docDto.name;
        this.middleName = docDto.middleName;
        this.surname = docDto.surname;
        this.gender = docDto.gender;
        this.dateOfBirth = docDto.dateOfBirth;
    }
}