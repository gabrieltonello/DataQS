using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQS.Core.Enums
{
    enum ValidationTypes
    {
        Unknown,            // Dado com validação desconhecida ou não classificada.
        GoodData,           // Dados de boa qualidade.
        PhysicallyInvalid,  // Dados fisicamente improváveis.
        RareData,           // Dados raros ou fora do comum, mas ainda plausíveis.
        SessionRelatedData  // Dados completos e relacionados a uma sessão, mas com possíveis incertezas.
    }
}
