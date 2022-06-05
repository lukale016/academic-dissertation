namespace models
{
    export interface Comment {
        owner: string;
        description: string;
        isDoctorComment: boolean;
    }
}