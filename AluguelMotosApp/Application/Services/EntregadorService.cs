using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EntregadorService : IEntregadorService
    {
        private readonly IEntregadorRepository _repository;
        private enum TiposDeCnhValidos
        {
            [Description("A")]
            A = 1,
            [Description("B")]
            B = 2,
            [Description("A+B")]
            AmaisB = 3
        }

        public EntregadorService(IEntregadorRepository repository)
        {
            _repository = repository;
        }

        public async Task AdicionarEntregadorAsync(EntregadorDto entregadorDto)
        {
            await ValidarSeJaExisteEntregadorComOMesmoCnpjOuCnh(entregadorDto);

            ValidarTipoDaCnh(entregadorDto.TipoCnh);

            //TODO: Usar AutoMapper
            var entregador = new Entregador()
            {
                Id = Guid.NewGuid(),
                Nome = entregadorDto.Nome,
                Cnpj = entregadorDto.Cnpj,
                DataNascimento = entregadorDto.DataNascimento,
                NumeroCnh = entregadorDto.NumeroCnh,
                TipoCnh = entregadorDto.TipoCnh,
                ImagemCnhPath = entregadorDto.ImagemCnhPath,
            };

            await _repository.AdicionarAsync(entregador);
        }

        private async Task ValidarSeJaExisteEntregadorComOMesmoCnpjOuCnh(EntregadorDto entregadorDto)
        {
            var existente = await _repository.ObterPorCnhOuCnpjAsync(entregadorDto.Cnpj, entregadorDto.NumeroCnh);

            if (existente != null)
                throw new InvalidOperationException("Já existe um entregador cadastrado com este CNPJ ou CNH.");
        }

        public async Task<Entregador> ObterPorIdAsync(Guid id)
        {
            var entregador = await _repository.ObterPorIdAsync(id);
            if (entregador == null)
                throw new KeyNotFoundException("Entregador não encontrado.");

            return entregador;
        }

        public async Task<IEnumerable<Entregador>> ObterTodosAsync()
        {
            return await _repository.ObterTodosAsync();
        }

        public async Task RemoverEntregadorAsync(Guid id)
        {
            var entregador = await _repository.ObterPorIdAsync(id);
            if (entregador == null)
                throw new KeyNotFoundException("Entregador não encontrado.");

            await _repository.RemoverAsync(id);
        }

        public bool ValidarEntregador(EntregadorDto entregadorDto)
        {
            return entregadorDto == null ||
                string.IsNullOrWhiteSpace(entregadorDto.Nome) ||
                string.IsNullOrWhiteSpace(entregadorDto.NumeroCnh) ||
                string.IsNullOrWhiteSpace(entregadorDto.Cnpj);
        }

        public static void ValidarTipoDaCnh(string tipoCnh)
        {
            if (string.IsNullOrWhiteSpace(tipoCnh))
                throw new ArgumentException("O tipo de CNH não pode ser vazio.");

            tipoCnh = tipoCnh.Trim().ToUpper();

            var tiposValidos = Enum.GetValues(typeof(TiposDeCnhValidos))
                                   .Cast<TiposDeCnhValidos>()
                                   .Select(v => ObterDescricaoDoEnum(v))
                                   .ToList();

            if (!tiposValidos.Contains(tipoCnh))
                throw new InvalidOperationException(
                    $"Tipo de CNH inválido. Tipos permitidos: {string.Join(", ", tiposValidos)}");
        }

        private static string ObterDescricaoDoEnum(Enum valor)
        {
            var campo = valor.GetType().GetField(valor.ToString());
            var attribute = (DescriptionAttribute)campo.GetCustomAttribute(typeof(DescriptionAttribute), false);
          
            return attribute == null ? valor.ToString() : attribute.Description;
        }
    }
}
