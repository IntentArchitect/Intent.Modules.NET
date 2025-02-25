namespace GrpcServer.GrpcServices;

partial class ComplexTypeDto
{
    internal Application.ComplexTypeDto FromMessage()
    {
        return new Application.ComplexTypeDto
        {
            Field = Field
        };
    }

    internal static ComplexTypeDto ToMessage(Application.ComplexTypeDto data)
    {
        return new ComplexTypeDto
        {
            Field = data.Field
        };
    }
}