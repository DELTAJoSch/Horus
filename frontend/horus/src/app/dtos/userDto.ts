export class User {
    name: string | undefined;
    email: string | undefined;
    role: number | undefined;
    password: string | undefined;

    roleName() : string {
        switch (this.role) {
            case 0:
                return 'Admin';
            case 1:
                return 'User';
            case 2:
                return 'Developer';
            default:
                return '';
        }
    }
}